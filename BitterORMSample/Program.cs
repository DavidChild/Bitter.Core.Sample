﻿using System;
using System.Collections.Generic;
using System.Data;
using Bitter.Core;
using BTORM.Sample.Test;

namespace Bitter.Sample.Test
{
    class Program
    {
        static void Main(string[] args)
        {
          

            // 根据ID 查询：
            var student = db.FindQuery<TStudentInfo>().QueryById(12);


            #region 根据条件全量查询  学生姓名为 HJB 的同学
            BList<TStudentInfo> students = db.FindQuery<TStudentInfo>().Where(p => p.FName == "HJB").Find();

            // 根据条件批量查询  学生姓名为 HJB 的同学
             TStudentInfo student_1 = db.FindQuery<TStudentInfo>().Where(p => p.FName == "HJB").Find().FirstOrDefault(); //此FirstOrDefault 重构过,为安全模式,数据库如果查不到数据，返回为空对象,避免返回 NULL.
            if (student_1.FID > 0) //说明查询到数据
            {

            }
            #endregion

            #region 根据条件全量查询 ,查询到年龄大于等于15 岁的学生 

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage >=15).Find();

            #endregion


            #region 根据条件全量查询 ,查询到年龄大于等于15 岁 并且 年龄 小于 17岁 的学生

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage >= 15&&p.FAage<17).Find();
            //上面的查询也可以写如下方式
            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage >= 15).Where(p => p.FAage < 17).Find(); //多级 Where 查询

            #endregion



            #region 根据条件全量查询 ,查询到年龄<>15

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage != 15).Find();

            #endregion


            #region 根据条件全量查询,查询到名字包含 "H" 的学生 

            students = db.FindQuery<TStudentInfo>().Where(p => p.FName.Contains("H")).Find(); //Contains 运行最终 Sql 为 ： '%H%'，暂时不支持 'H%','%H'

            #endregion


            #region 根据条件全量查询,多级 Where  查询

            var studentquery = db.FindQuery<TStudentInfo>().Where(p => p.FName.Contains("H")); //Contains 运行最终 Sql 为 ： '%H%'，暂时不支持 'H%','%H'
            studentquery.Where(p=>p.FAage>15);
            students = studentquery.Find();

            #endregion

            #region 根据条件全量查询 ,查询到年龄<>15 的总数量

            var count = db.FindQuery<TStudentInfo>().Where(p => p.FAage != 15).FindCount();

            #endregion


            #region  ASC  正序 

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAddTime).Find();

            #endregion


            #region  DESC  正序 

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenDesc(p => p.FAddTime).Find();

            #endregion



            #region  DESC  多级正序 

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenDesc(p => p.FAage).ThenDesc(p => p.FAddTime).Find();

            #endregion

            #region  ASC  多级正序 

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenAsc(p => p.FAddTime).Find();

            #endregion

            #region  (ASC-DESC)  多级正序倒序混排

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).Find();

            #endregion

            #region  单表模型驱动查询--只查询符合条件的前 N 条数据，并且只返回具体的列（FAage,FName）：

            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).SetSize(10).Select(c=>new object[] { c.FAage,c.FName}).Find(); //后面的 SetSize(N)  方法指定了需要查询的前N 条数量
            students = db.FindQuery<TStudentInfo>().Where(p => p.FAage > 15).ThenAsc(p => p.FAage).ThenDesc(p => p.FAddTime).SetSize(10).Select(c => new List<object>{ c.FAage, c.FName }).Find(); //后面的 SetSize(N)  方法指定了需要查询的前N 条数量

            #endregion


            #region 高级查询直接SQL语句查询（非分页）
            //查出分数>=90分的学生姓名以及具体学分
            DataTable dt=  db.FindQuery(@"SELECT score.FScore,student.FName as studentNameFROM  t_StudentScore score
                                LEFT JOIN t_student student  ON score.FStudentId = student.FID
                                where score.FScore>=@score
                              ", new { score = 90 }).Find();


            #endregion


            #region 分页查询示例
            List<TScoreSearchDto> list = getScoreList(1,"H");
            #endregion


            #region 插入、删除、更新示例
            DemoForOp();
            DemoFopOpByDirectSql(11);
            #endregion


            #region //WITH 子句的支持
            With_Page_Demo(1, "H");
            #endregion

            Console.ReadKey();


        }




      
        /// <summary>
        /// 插入，删除，更新示例(模型驱动)
        /// </summary>

        public static void DemoForOp()
        {
            var d = db.FindQuery<TStudentInfo>().Where(p => p.FName == "DavidChild").Find().FirstOrDefault(); 
            if (d.FID > 0)
            {
                d.FAage = 18;
               int isupdateSuccess= d.Update().Submit();//更新操作：//成功 返回受影响的行数，操作异常:返回-1。  Submit() 是必须的，只有Submit() 后，才能持久化数据库层面。
                if (isupdateSuccess < 0)
                {
                    //更新失败了，具体异常原因可以看日志
                }
                else
                {
                    //更新成功 ，注意：isdeletesuccess为零的时候，执行语句没有异常，只是数据库返回的受影响行数为零。
                }
            }
            else
            {
                d.FName = "DavidChild";
                d.FClassId = 1;
                d.FAddTime = DateTime.Now;
                d.FAage = 18;
                int insertidentity=  d.Insert().Submit(); //非常重要(Notic)：如果插入异常，返回是-1,如果成功,返回主键Id. Submit() 是必须的，只有Submit() 后，才能持久化数据库层面。
                if (insertidentity <= 0)
                {
                    //插入失败了,具体异常原因可以看日志
                }
                else
                {

                    var ndata = db.FindQuery<TStudentInfo>().QueryById(insertidentity); //重新查询最新插入的数据
                }
           }

            int  isdeletesuccess= d.Delete().Submit();//删除,返回受影响的行数 
            if (isdeletesuccess < 0) 
            {
                //删除异常（失败）了，具体异常原因可以看日志
            }
            else
            {
                //删除成功 ，注意：isdeletesuccess为零的时候，执行语句没有异常，只是数据库返回的受影响行数为零。
            }

        }



        /// <summary>
        /// 直接使用SQL 语句来操作数据库示例
        /// </summary>

        public static void DemoFopOpByDirectSql(int id)
        {
            int isdealsuccess= db.Excut("update t_student set fname='DavidChild' where FId=@Id;", new { FId = id }).Submit();
            if (isdealsuccess < 0)
            {
                //Sql 操作执行（异常）失败了，具体异常原因可以看日志
            }
            else
            {
                //执行成功 ，注意：isdealsuccess为零的时候，执行语句没有异常，只是数据库返回的受影响行数为零。
            }
        }



        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        public static List<TScoreSearchDto> getScoreList(int type,string studentname)
        {
            #region //聚联条件分页查询 


            var sql = @"SELECT   score.FScore,student.FName  as studentName,class.FName as className,grade.FName as  gradeName FROM  dbo.t_StudentScore score
            LEFT JOIN dbo.t_student student  ON score.FStudentId = student.FID
            LEFT JOIN dbo.t_class class ON  student.FClassId=class.FID
            LEFT  JOIN dbo.t_Grade grade ON  grade.FID=class.FGradeId
            ";
            PageQuery pq = new PageQuery(sql, null);

            pq.Where("1=1");

            if (type == 1)
            {
                pq.Where("score.FScore>60 ");
            }
            if (type == 2)
            {
                pq.Where("score.FScore>60 and score.FScore<80 ");
            }
            if (!string.IsNullOrEmpty(studentname))
            {
                pq.Where(" student.FName like '%' + @FScoreName + '%'",new {FScoreName=studentname});
            }

            //通过ThenAsc 方法指定字段排序
            pq.ThenASC("score.FScore ");

            //通过ThenDESC 方法指定字段排序
            pq.ThenDESC("student.FName");

            //自己直接指定排序字段和排序关键词
            pq.OrderBy("student.FAddTime desc");

            //分页指定 Skip: 当前页，Take :每页数量
            pq.Skip(1).Take(10);

            var dt = pq.ToDataTable(); //获取数据

            var studentscount = pq.Count(); //获取当前条件下的数量


            return dt.ToListModel<TScoreSearchDto>(); //ToList<T>() DataTable-->List<T> 的模型转换


            #endregion

        }

        /// <summary>
        /// 分页查询--WITH 子句的支持
        /// </summary>
        /// <returns></returns>
        public static List<TScoreSearchDto> With_Page_Demo(int type, string studentname)
        {
            #region //聚联条件分页查询  年级的LEFT JOIN 子语句 使用WITH 来替代

            var withsql = @"WITH grade as(select* from t_Grade where fid=@id)"; //定义WITH 子句
            var sql = @"SELECT   score.FScore,student.FName  as studentName,class.FName as className,grade.FName as  gradeName FROM  dbo.t_StudentScore score
            LEFT JOIN dbo.t_student student  ON score.FStudentId = student.FID
            LEFT JOIN dbo.t_class class ON  student.FClassId=class.FID
            LEFT JOIN grade ON grade.FID=class.FGradeId";

            PageQuery pq = new PageQuery(sql, null);
            pq.AddPreWith(withsql, new { id = 3 });
           


            pq.Where("1=1");

            if (type == 1)
            {
                pq.Where("score.FScore>60 ");
            }
            if (type == 2)
            {
                pq.Where("score.FScore>60 and score.FScore<80 ");
            }
            if (!string.IsNullOrEmpty(studentname))
            {
                pq.Where(" student.FName like '%' + @FScoreName + '%'", new { FScoreName = studentname });
            }

            //通过ThenAsc 方法指定字段排序
            pq.ThenASC("score.FScore ");

            //通过ThenDESC 方法指定字段排序
            pq.ThenDESC("student.FName");

            //自己直接指定排序字段和排序关键词
            pq.OrderBy("student.FAddTime desc");

            //分页指定 Skip: 当前页，Take :每页数量
            pq.Skip(1).Take(10);

            var dt = pq.ToDataTable(); //获取数据

            var studentscount = pq.Count(); //获取当前条件下的数量


            return dt.ToListModel<TScoreSearchDto>(); //ToList<T>() DataTable-->List<T> 的模型转换


            #endregion

        }



        /// <summary>
        /// 事务代码详细教程
        /// </summary>
        public static void DbScopeDemo()
        {
            dbscope dbs = new dbscope(); //示例化事务收集器
            //说明：事务收集器 dbscope.dotrancation 的参数是一个匿名委托方法
            dbs.dotrancation((list, models, bulkcopymodels) =>
            {
                TGRADEInfo gRadeinfo = new TGRADEInfo();
                gRadeinfo.FName = "年级_" + 1;
                gRadeinfo.FAddTime = DateTime.Now;
                var gradid = gRadeinfo.AddInScope(models); /**
                                                    * 如果 下面的代码需要用到新增数据的在数据库的自增种子ID，并且又需要通过事务执行。怎么办？
                                                    * 如 gRadeinfo  对象是一个年级新增实例，但是下面的 classInfo 班级新增实例 中的  FGradeId 关联了  gRadeinfo 中的 主键ID。
                                                    * 那么新增模型 可以通过 db.dotrancation 的匿名委托方法中通过 AddInScope 方法操作， 
                                                    * 在事务中先将先执行gRadeinfo执行到数据库中,获取到ID，然后将此模型缓存起来，
                                                    * 如果事务执行失败，那么此模型行将自动执行删除操作！
                                                  */
                if (gradid <= 0)
                {
                    throw (new Exception("错误：终止事务！")); 
                }

                TClassInfo classInfo = new TClassInfo();
                classInfo.FName = "班级201";
                Random rd = new Random();
                classInfo.FGradeId = gradid; //使用上面新增的 gRadeinfo 数据库新产生的 自增长主键Id 
                classInfo.FAddTime = DateTime.Now;
                classInfo.Insert().AddInScope(list); // 塞入 到  list-- sqlquery 收集器中 中，等待提交执行

                 //学生
                var count = 20;
                for (int ci = 0; ci <= count; ci++)
                {
                    TStudentInfo info = new TStudentInfo();
                    info.FName = "HJB" + ci;


                    info.FClassId = 2;
                    info.FAddTime = DateTime.Now;
                    Random rdage = new Random();
                    info.FAage = rdage.Next(16, 20);
                    info.BulkCopy().AddInScope(bulkcopymodels);//塞入 到  bulkcopymodels-- bulkcopy 收集器 等待提交执行

                }



                var sqlcommand = "update t_student set FAge=@age";
                db.Excut(sqlcommand,new { age=17 }).AddInScope(list); //将裸SQL的操作执行放入 listlist-- sqlquery 收集器中等待执行

                var stduent_1 = db.FindQuery<TStudentInfo>().QueryById(50);
                stduent_1.FAage = 16;
                stduent_1.Update().AddInScope(list); // 塞入 到  list-- sqlquery 收集器中 中，等待提交执行


                var stduent_2 = db.FindQuery<TStudentInfo>().QueryById(51);
                stduent_2.FAage = 18;
                stduent_2.Delete().AddInScope(list); // 塞入 到  list-- sqlquery 收集器中 中，等待提交执行
            });
            bool issuccess= dbs.Submit();
            if (issuccess)
            {
                //事务提交执行成功
            }
            else
            {
                //事务提交失败
                string failmessage = dbs.ScopeException.Message;
            }
        }


          
          


            /// <summary>
            /// 初始化数据
            /// </summary>
        public static void InitData()
        {
            int count = 11;
            int ix = 0;
            //年级
            for (ix = 0; ix <= count; ix++)
            {
                TGRADEInfo info = new TGRADEInfo();
                info.FName = "年级_" + ix;
                info.FAddTime = DateTime.Now;
                info.Insert().Submit();

            }
            //班级
            for (ix = 0; ix <= count; ix++)
            {
                TClassInfo info = new TClassInfo();
                info.FName = "班级_" + ix;
                Random rd = new Random();

                info.FGradeId = rd.Next(1, 10);
                info.FAddTime = DateTime.Now;
                info.Insert().Submit();


            }
            //学生
            count = 1000;
            for (ix = 0; ix <= count; ix++)
            {
                TStudentInfo info = new TStudentInfo();
                info.FName = "HJB" + ix;
                Random rd = new Random();
                info.FClassId = rd.Next(2, 9);
                info.FAddTime = DateTime.Now;
                Random rdage = new Random();
                info.FAage = rdage.Next(16, 20);
                info.Insert().Submit();

            }

            //学分
            count = 2000;
            for (ix = 0; ix <= count; ix++)
            {
                TStudentScoreInfo info = new TStudentScoreInfo();

                Random rd = new Random();
                info.FStudentId = rd.Next(2, 99);

                info.FAddTime = DateTime.Now;
                Random rdage = new Random();
                info.FScore = rdage.Next(1, 100);
                info.Insert().Submit();

            }

        }

    }
}
