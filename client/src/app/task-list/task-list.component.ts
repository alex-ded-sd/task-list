import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTable } from "@angular/material/table";
import { TaskListDataSource, TaskListItem } from "./task-list-datasource";

@Component({
	selector: "app-task-list",
	templateUrl: "./task-list.component.html",
	styleUrls: ["./task-list.component.css"]
})
export class TaskListComponent implements AfterViewInit, OnInit {
	@ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
	@ViewChild(MatSort, { static: false }) sort: MatSort;
	@ViewChild(MatTable, { static: false }) table: MatTable<TaskListItem>;
	dataSource: TaskListDataSource;

	/** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
	displayedColumns = ["id", "name", "actions"];

	ngOnInit() {
		this.dataSource = new TaskListDataSource();
	}

	ngAfterViewInit() {
		this.dataSource.sort = this.sort;
		this.dataSource.paginator = this.paginator;
		this.table.dataSource = this.dataSource;
	}
}
